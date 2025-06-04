from fastapi import APIRouter, HTTPException, status, Depends, Response
from src.database import engine, Base
from src.models.users import UsersModel
from src.models.spare_parts import SparePartsModel
from src.models.maintenances import MaintenancesModel
from src.models.clients import ClientsModel
from src.models.checks import ChecksModel


router = APIRouter(prefix="/database")

@router.post('/setup_db', tags=["База данных"], summary=["Инициализация БД"])
async def setup_db():
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.drop_all)
        await conn.run_sync(Base.metadata.create_all)
        return {"message": "Database setup successfully"}