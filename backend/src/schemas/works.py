from pydantic import BaseModel


class WorksSchema(BaseModel):
    work_type: str
    date: str
    deadline: str
    next_maintenance: str
    comment: str

    