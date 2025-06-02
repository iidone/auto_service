from pydantic import BaseModel


class MaintenancesSchema(BaseModel):
    user_id: int
    car_id: int
    work_id: int
    date: str
    next_maintenance: str
    comment: str