from fastapi import APIRouter
from src.api.endpoints.users import router as users_router
from src.api.endpoints.maintenances import router as maintenances_router



main_router = APIRouter()

main_router.include_router(users_router)
main_router.include_router(maintenances_router)