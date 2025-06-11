from typing_extensions import List
from fastapi import APIRouter, HTTPException, status, Depends, Response
from sqlalchemy import select
from src.models.clients import ClientsModel
from src.schemas.clients import ClientsSchema, ClientCreate
from src.api.dependencies import SessionDep


router = APIRouter(prefix="/clients")

@router.post("/add_client", response_model=ClientCreate, status_code=status.HTTP_201_CREATED, tags=["Клиенты"], summary=["Добавить клиента"])
async def add_client(client_data: ClientsSchema, session: SessionDep):
    try:
        
        
        new_client = ClientsModel(
            first_name = client_data.first_name,
            last_name = client_data.last_name,
            contact = client_data.contact,
            brand = client_data.brand,
            series = client_data.series,
            number = client_data.number,
            mileage = client_data.mileage,
            age = client_data.age,
            vin = client_data.vin,
            last_maintenance = client_data.last_maintenance
        )
        session.add(new_client)
        await session.commit()
        await session.refresh(new_client)
        
        return new_client
    
    except HTTPException:
        raise
    except Exception as e:
        await session.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating client: {str(e)}"
        )
    

@router.get("/all_clients", response_model=List[ClientsSchema], tags=["Клиенты"], summary=["Получить всех клиентов"])
async def get_all_clients(session: SessionDep):
    try:
        result = await session.execute(select(ClientsModel))
        clients = result.scalars().all()
        return clients
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")