from pydantic import BaseModel


class WorksSchema(BaseModel):
    work_type: str
    deadline: str

    