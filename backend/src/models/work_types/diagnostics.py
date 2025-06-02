from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class DiagnosticsModel(Base):
    __tablename__ = "diagnostics"

    id: Mapped[int] = mapped_column(primary_key=True)
    engine: Mapped[str] = mapped_column()
    suspension: Mapped[str] = mapped_column()
    brakes: Mapped[str] = mapped_column()
    transmission: Mapped[str] = mapped_column()
    electronics: Mapped[str] = mapped_column()
    body_and_frame: Mapped[str] = mapped_column()
    air_conditioning_and_heating: Mapped[str] = mapped_column()
