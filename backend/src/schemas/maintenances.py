from pydantic import BaseModel, ConfigDict


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