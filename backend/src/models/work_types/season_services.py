from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class SeasonServicesModel(Base):
    __tablename__ = "season_services"

    id: Mapped[int] = mapped_column(primary_key=True)
    change_tires_to: Mapped[str] = mapped_column()
