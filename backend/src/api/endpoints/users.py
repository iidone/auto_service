from typing_extensions import List
from fastapi import APIRouter, HTTPException, status, Depends, Response
from fastapi.security import OAuth2PasswordRequestForm
from sqlalchemy import select, delete
from src.models.users import UsersModel
from src.schemas.users import DeleteMasterRequest, UsersSchema, UserResponce, UserCreate
from src.api.dependencies import (
    add_to_blacklist,
    pwd_context, 
    SessionDep,
    oauth2_scheme,
    create_access_token,
    authenticate_user,
    token_blacklist,
    verify_password
)

router = APIRouter(prefix="/users")

@router.post("/login", tags=["Пользователи"],summary = ["Авторизация"])
async def login_user(
    session: SessionDep,
    form_data: OAuth2PasswordRequestForm = Depends()
):
    user = await authenticate_user(form_data.username, form_data.password, session)
    if not user:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Incorrect username or password",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    access_token = create_access_token(data={"sub": user.username})
    
    return {
        "access_token": access_token,
        "token_type": "bearer",
        "user_info": {
            "username": user.username,
            "user_id": user.id,
            "user_role": user.role,
            "first_name": user.first_name,
        }
    }

@router.post("/logout", tags=["Пользователи"], summary=["Выход"])
async def logout_user(
    session: SessionDep,
    response: Response,
    token: str = Depends(oauth2_scheme),
):
    try:
        await add_to_blacklist(token)

        response.delete_cookie("access_token")
        
        return {
            "message": "Logout successful",
            "detail": "Token invalidated. Client should discard the token."
        }
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Logout error: {str(e)}"
        )
    



@router.post("/add_user", response_model=UserCreate, status_code=status.HTTP_201_CREATED, tags=["Пользователи"], summary=["Добавить пользователя"])
async def add_user(user_data: UsersSchema, session: SessionDep):
    try:
        existing_user = await session.execute(
            select(UsersModel).where(UsersModel.username == user_data.username)
        )
        if existing_user.scalar_one_or_none():
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="Username already exist."
            )
        new_user = UsersModel(
            role=user_data.role,
            username=user_data.username,
            first_name=user_data.first_name,
            last_name=user_data.last_name,
            password=pwd_context.hash(user_data.password),
            contact=user_data.contact,
        )
        session.add(new_user)
        await session.commit()
        await session.refresh(new_user)
        
        return new_user
    
    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating user: {str(e)}"
        )




@router.get("/all_users", response_model=List[UserResponce], tags=["Пользователи"], summary=["Получить всех пользователей"])
async def get_all_users(session: SessionDep):
    try:
        result = await session.execute(select(UsersModel))
        users = result.scalars().all()
        return users
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")
    
    

@router.get("/all_masters", response_model=List[UserResponce], tags=["Пользователи"], summary=["Получить всех мастеров"])
async def get_all_masters(session: SessionDep):
    try:
        result = await session.execute(select(UsersModel).where(UsersModel.role == "master"))
        users = result.scalars().all()
        return users
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")
    

@router.get("/all_managers", response_model=List[UserResponce], tags=["Пользователи"], summary=["Получить всех менеджеров"])
async def get_all_managers(session: SessionDep):
    try:
        result = await session.execute(select(UsersModel).where(UsersModel.role == "manager"))
        users = result.scalars().all()
        return users
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")
    

@router.post(
    "/delete-many",
    status_code=status.HTTP_200_OK,
    response_model=dict,
    tags=["Пользователи"],
    summary=["Удалить нескольких пользователей"]
)
async def delete_master(
    session: SessionDep,
    request: DeleteMasterRequest,
):
    try:
        if not request.ids:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="Список ID не может быть пустым"
            )

        stmt = select(UsersModel).where(
            UsersModel.id.in_(request.ids),
            UsersModel.role == "master"
        )
        result = await session.execute(stmt)
        masters_to_delete = result.scalars().all()

        if not masters_to_delete:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail="Не найдено мастеров с указанными ID"
            )

        master_ids = [master.id for master in masters_to_delete]

        await session.execute(
            delete(UsersModel).where(UsersModel.id.in_(master_ids)))
        await session.commit()

        return {
            "message": f"Удалено мастеров: {len(master_ids)}",
            "deleted_ids": master_ids
        }
    

    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при удалении мастеров: {str(e)}"
        )
    
