from pydantic import BaseModel, ConfigDict


class ClientsSchema(BaseModel):
    first_name: str
    last_name: str
    contact: str
    brand: str
    series: str
    number: str
    mileage: str
    age: str
    vin: str
    last_maintenance: str
    model_config = ConfigDict(from_attributes=True)
    
    
class ClientCreate(ClientsSchema):
    pass

class ClientResponce(ClientsSchema):
    id: int