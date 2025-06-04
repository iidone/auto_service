from fastapi import APIRouter
from src.api.endpoints.users import router as users_router
from src.api.endpoints.database import router as database_router



main_router = APIRouter()

main_router.include_router(users_router)
main_router.include_router(database_router)