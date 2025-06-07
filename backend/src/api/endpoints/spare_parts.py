from pydantic import BaseModel
from typing import List
from fastapi import APIRouter, HTTPException, status, Depends
from sqlalchemy import delete, select
from src.models.spare_parts import SparePartsModel
from src.schemas.spare_parts import SparePartsSchema, DeleteSparePartsRequest
from src.api.dependencies import SessionDep

router = APIRouter(prefix="/spare-parts")



@router.post("/add_spare_parts", response_model=List[SparePartsSchema], status_code=status.HTTP_201_CREATED, tags=["Добавить запчасть"])
async def add_spare_part(spare_parts_data: SparePartsSchema, session: SessionDep):
    try:
        new_spare_part = SparePartsModel(
            title = spare_parts_data.title,
            category = spare_parts_data.category,
            article = spare_parts_data.article,
            analog = spare_parts_data.analog,
            supplier = spare_parts_data.supplier,
            price = spare_parts_data.price
        )
        session.add(new_spare_part)
        await session.commit()
        await session.refresh(new_spare_part)
        
        return new_spare_part
    
    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating spare part: {str(e)}"
        )



@router.delete(
    "/delete-many",
    status_code=status.HTTP_200_OK,
    response_model=dict,
    tags=["Удаление запчастей"]
)
async def delete_spare_parts(
    request: DeleteSparePartsRequest,
    session: SessionDep):
    try:
        if not request.ids:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="Список ID не может быть пустым"
            )

        result = await session.execute(
            select(SparePartsModel.id).where(SparePartsModel.id.in_(request.ids)))
        existing_ids = result.scalars().all()

        if not existing_ids:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail="Не найдено запчастей с указанными ID"
            )

        await session.execute(
            delete(SparePartsModel).where(SparePartsModel.id.in_(existing_ids)))
        await session.commit()

        return {
            "message": f"Успешно удалено запчастей: {len(existing_ids)}",
            "deleted_ids": existing_ids
        }

    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при удалении запчастей: {str(e)}"
        )
    

@router.get("/all_spare_parts", response_model=List[SparePartsSchema], tags=["Все запчасти"])
async def get_all_spare_parts(session: SessionDep):
    try:
        result = await session.execute(select(SparePartsModel))
        spare_parts = result.scalars().all()
        return spare_parts
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")