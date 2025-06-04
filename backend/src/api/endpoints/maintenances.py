from typing_extensions import List
from fastapi import APIRouter, HTTPException, status, Depends, Response
from sqlalchemy import select
from src.models.maintenances import MaintenancesModel
from src.schemas.maintenances import MaintenancesSchema
from src.api.dependencies import (
    SessionDep,
)


router = APIRouter(prefix="/maintenances")

# ПОЧИНИТЬ
@router.post("/add_maintenance", response_model=List[MaintenancesSchema], status_code=status.HTTP_201_CREATED, tags=["Добавить ТО"])
async def add_maintenance(maintenance_data: MaintenancesSchema, session: SessionDep):
    try:

        new_maintenance = MaintenancesModel(
            user_id = maintenance_data.user_id,
            client_id = maintenance_data.client_id,
            description = maintenance_data.description,
            date = maintenance_data.date,
            next_maintenance = maintenance_data.next_maintenance,
            comment = maintenance_data.comment,
            price = maintenance_data.price
        )
        session.add()
        await session.commit()
        await session.refresh(new_maintenance)
        
        return new_maintenance
    
    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating user: {str(e)}"
        )
