from pydantic import BaseModel
from typing_extensions import List
from fastapi import APIRouter, HTTPException, Path, status, Depends, Response
from sqlalchemy import select
from src.models.maintenances import MaintenancesModel
from src.schemas.maintenances import MaintenancesSchema
from src.api.dependencies import (
    SessionDep,
)
from src.models.clients import ClientsModel
from src.schemas.clients import ClientsSchema



router = APIRouter(prefix="/maintenances")


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



@router.get("/all_maintenances", response_model=List[MaintenancesSchema], tags=["Все ТО"])
async def get_all_maintenances(session: SessionDep):
    try:
        result = await session.execute(select(MaintenancesModel))
        maintenances = result.scalars().all()
        return maintenances
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")
    



class MaintenanceWithClient(BaseModel):
    maintenance: MaintenancesSchema
    client: ClientsSchema

@router.get("/maintenances_with_clients", 
           response_model=List[MaintenanceWithClient],
           tags=["Все ТО с информацией о клиентах"])
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

router = APIRouter(prefix="/maintenances")

@router.get("/by_master/{master_id}",
           response_model=List[MaintenanceWithClientResponse],
           tags=["ТО мастера с клиентами"])
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