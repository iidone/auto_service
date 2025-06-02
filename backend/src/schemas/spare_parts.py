from pydantic import BaseModel


class CarsSchema(BaseModel):
    title: str
    category: str
    article: str
    analog: str
    supplier: str
    deadlines: str
    price: str
    