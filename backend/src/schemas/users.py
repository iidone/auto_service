from pydantic import BaseModel


class UsersSchema(BaseModel):
    role: str
    username: str
    first_name: str
    last_name: str
    password: str
    contact: str
