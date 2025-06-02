from typing_extensions import List
from fastapi import APIRouter, HTTPException, status
from sqlalchemy import select
from src.schemas.users import UsersSchema
from src.api.dependencies import SessionDep
from src.models.users import UsersModel


router = APIRouter(prefix="/users")


@router.post("/v1", response_model=List[UsersSchema], status_code=status.HTTP_201_CREATED, tags=["Добавить пользователя"])
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
            username=user_data.username,
            first_name=user_data.first_name,
            last_name=user_data.last_name,
            password=user_data.password,
            contact=user_data.contact,
            vin=user_data.vin
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

@router.get("/v2", response_model=List[UsersSchema], tags=["Все пользователи"])
async def get_all_users(session: SessionDep):
    try:
        result = await session.execute(select(UsersModel))
        users = result.scalars().all()
        return users
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")