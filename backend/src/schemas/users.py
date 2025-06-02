from pydantic import BaseModel


class UsersSchema(BaseModel):
    username: str
    first_name: str
    last_name: str
    password: str
    contact: str
    vin: str