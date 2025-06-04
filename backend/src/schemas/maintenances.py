from pydantic import BaseModel


class MaintenancesSchema(BaseModel):
    user_id: int
    client_id: int
    description: str
    date: str
    next_maintenance: str
    comment: str
    price: str