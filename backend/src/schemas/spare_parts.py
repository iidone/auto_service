from pydantic import BaseModel
from typing_extensions import List


class SparePartsSchema(BaseModel):
    title: str
    category: str
    article: str
    analog: str
    supplier: str
    price: str
    
class DeleteSparePartsRequest(BaseModel):
    ids: List[int] 