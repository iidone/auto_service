from pydantic import BaseModel


class SparePartsSchema(BaseModel):
    title: str
    category: str
    article: str
    analog: str
    supplier: str
    price: str
    