from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class MaintenancesModel(Base):
    __tablename__ = "maintenances"

    id: Mapped[int] = mapped_column(primary_key=True, autoincrement=True)
    user_id: Mapped[int] = mapped_column()
    client_id: Mapped[int] = mapped_column()
    description: Mapped[str] = mapped_column()
    date: Mapped[str] = mapped_column()
    next_maintenance: Mapped[str] = mapped_column()
    comment: Mapped[str] = mapped_column()
    status: Mapped[str] = mapped_column(default="waiting")
    price: Mapped[str] = mapped_column()