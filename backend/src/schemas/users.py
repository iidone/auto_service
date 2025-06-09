from pydantic import BaseModel
from typing_extensions import List

class UsersSchema(BaseModel):
    id: int
    role: str
    username: str
    first_name: str
    last_name: str
    password: str
    contact: str


class DeleteMasterRequest(BaseModel):
    ids: List[int]