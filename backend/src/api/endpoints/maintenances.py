from pydantic import BaseModel
from typing_extensions import List
from fastapi import APIRouter, HTTPException, Path, status, Depends, Response
from sqlalchemy import select, delete
from src.models.maintenances import MaintenancesModel
from src.schemas.maintenances import DeleteMaintenanceRequest, MaintenancesSchema, MaintenanceStatusUpdate, MaintenanceCreate, MaintenanceResponce
from src.api.dependencies import (
    SessionDep,
)
from src.models.clients import ClientsModel
from src.schemas.clients import ClientsSchema



router = APIRouter(prefix="/maintenances")


@router.post("/add_maintenance", response_model=MaintenanceCreate, status_code=status.HTTP_201_CREATED, tags=["Работы"], summary=["добавить ТО"])
async def add_maintenance(maintenance_data: MaintenanceCreate, session: SessionDep):
    try:

        new_maintenance = MaintenancesModel(
            user_id = maintenance_data.user_id,
            client_id = maintenance_data.client_id,
            description = maintenance_data.description,
            date = maintenance_data.date,
            next_maintenance = maintenance_data.next_maintenance,
            comment = maintenance_data.comment,
            status = "waiting",
            price = maintenance_data.price
        )
        session.add(new_maintenance)
        await session.commit()
        await session.refresh(new_maintenance)
        
        return new_maintenance
    
    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating maintenance: {str(e)}"
        )



@router.get("/all_maintenances", response_model=List[MaintenanceResponce], tags=["Работы"], summary=["Получить все работы"])
async def get_all_maintenances(session: SessionDep):
    try:
        result = await session.execute(select(MaintenancesModel))
        maintenances = result.scalars().all()
        return maintenances
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")
    



class MaintenanceWithClient(BaseModel):
    maintenance: MaintenanceResponce
    client: ClientsSchema

@router.get("/maintenances_with_clients", 
           response_model=List[MaintenanceWithClient],
           tags=["Работы"],
           summary=["Получить все работы с клиентами"])
async def get_maintenances_with_clients(session: SessionDep):
    try:
        query = select(MaintenancesModel, ClientsModel).\
                join(ClientsModel, MaintenancesModel.client_id == ClientsModel.id)
        
        result = await session.execute(query)

        response = []
        for maintenance, client in result:
            response.append({
                "maintenance": maintenance,
                "client": client
            })
        
        return response
    
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Database error: {str(e)}"
        )
    



class MaintenanceWithClientResponse(BaseModel):
    maintenance: MaintenancesSchema
    client: ClientsSchema


@router.get("/by_master/{master_id}",
           response_model=List[MaintenanceWithClientResponse],
           tags=["Работы"],
           summary=["Получить все работы по мастеру"])
async def get_maintenances_by_master(
    session: SessionDep,
    master_id: int = Path(..., title="ID мастера"),
):
    try:
        query = (
            select(MaintenancesModel, ClientsModel)
            .join(ClientsModel, MaintenancesModel.client_id == ClientsModel.id)
            .where(MaintenancesModel.user_id == master_id)
        )
        
        result = await session.execute(query)
        
        response = []
        for maintenance_model, client_model in result:
            maintenance = MaintenancesSchema.from_orm(maintenance_model)
            client = ClientsSchema.from_orm(client_model)
            response.append(MaintenanceWithClientResponse(
                maintenance=maintenance,
                client=client
            ))
        
        if not response:
            raise HTTPException(
                status_code=404,
                detail=f"No maintenances found for master with id {master_id}"
            )
        
        return response
    
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Database error: {str(e)}"
        )
    

@router.post(
    "/delete-many",
    status_code=status.HTTP_200_OK,
    response_model=dict,
    tags=["Работы"],
    summary=["Удалить несколько работ"])
async def delete_maintenance(
    session: SessionDep,
    request: DeleteMaintenanceRequest,
    ):
    try:
        if not request.ids:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="Список ID не может быть пустым"
            )

        result = await session.execute(
            select(MaintenancesModel.id).where(MaintenancesModel.id.in_(request.ids)))
        existing_ids = result.scalars().all()

        if not existing_ids:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail="Не найдено ТО с указанными ID"
            )

        await session.execute(
            delete(MaintenancesModel).where(MaintenancesModel.id.in_(existing_ids)))
        await session.commit()

        return {
            "message": f"Успешно удалено ТО: {len(existing_ids)}",
            "deleted_ids": existing_ids
        }

    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при удалении ТО: {str(e)}"
        )
        
        
@router.patch(
    "/update_maintenance_status/{maintenance_id}",
    response_model=MaintenancesSchema,
    tags=["Работы"],
    summary="Обновить статус заявки"
)
async def update_maintenance_status(
    maintenance_id: int,
    status_update: MaintenanceStatusUpdate,
    session: SessionDep
):
    try:
        maintenance = await session.execute(
            select(MaintenancesModel)
            .where(MaintenancesModel.id == maintenance_id)
        )
        maintenance = maintenance.scalar_one_or_none()
        if not maintenance:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail="Заявка не найдена"
            )
        valid_statuses = ["waiting", "in_progress", "completed", "cancelled"]
        if status_update.status not in valid_statuses:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=f"Недопустимый статус. Допустимые значения: {', '.join(valid_statuses)}"
            )
        
        maintenance.status = status_update.status
        await session.commit()
        await session.refresh(maintenance)
        
        return maintenance
    
    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при обновлении статуса: {str(e)}"
        )