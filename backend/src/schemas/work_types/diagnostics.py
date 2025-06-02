from pydantic import BaseModel


class DiagnosticsSchema(BaseModel):
    engine: str
    suspension: str
    brakes: str
    transmission: str
    electronics: str
    body_and_frame: str
    air_conditioning_and_heating: str
