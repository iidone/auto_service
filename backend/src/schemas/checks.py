from pydantic import BaseModel


class MaintenancesSchema(BaseModel):
    maintenance_id: int
    spare_parts_id: int
    total_price: str