from pydantic import BaseModel


class ClientsSchema(BaseModel):
    first_name: str
    last_name: str
    contact: str
    brand: str
    series: str
    number: str
    mileage: str
    age: str
    maintenance_id: int
    vin: str
    last_maintenance: str