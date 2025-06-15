from pydantic import BaseModel, ConfigDict
from enum import Enum
from typing_extensions import List


class MaintenancesSchema(BaseModel):
    user_id: int
    client_id: int
    description: str
    date: str
    next_maintenance: str
    comment: str
    status: str
    price: str
    model_config = ConfigDict(from_attributes=True)


class DeleteMaintenanceRequest(BaseModel):
    ids: List[int]
    
    
class MaintenanceCreate(MaintenancesSchema):
    pass

class MaintenanceResponce(MaintenancesSchema):
    id: int
    
class MaintenanceStatusAndPriceUpdate(BaseModel):
    status: str
    price: str
    
    class Config:
        schema_extra = {
            "example":
                {
                    "status": "in_progress"
                }
        }