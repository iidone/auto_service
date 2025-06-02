from pydantic import BaseModel


class CarsSchema(BaseModel):
    brand: str
    series: str
    number: str
    mileage: str
    age: str
    maintenance_id: int