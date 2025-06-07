from fastapi import APIRouter
from src.api.endpoints.users import router as users_router
from src.api.endpoints.database import router as database_router
from src.api.endpoints.maintenances import router as maintenances_router
from src.api.endpoints.clients import router as clients_router
from src.api.endpoints.spare_parts import router as spare_parts_router


main_router = APIRouter()

main_router.include_router(users_router)
main_router.include_router(database_router)
main_router.include_router(maintenances_router)
main_router.include_router(clients_router)
main_router.include_router(spare_parts_router)