from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class ClientsModel(Base):
    __tablename__ = "clients"

    id: Mapped[int] = mapped_column(primary_key=True, autoincrement=True)
    first_name: Mapped[str] = mapped_column()
    last_name: Mapped[str] = mapped_column()
    contact: Mapped[str] = mapped_column()
    brand: Mapped[str] = mapped_column()
    series: Mapped[str] = mapped_column()
    number: Mapped[str] = mapped_column(unique=True)
    mileage: Mapped[str] = mapped_column()
    age: Mapped[str] = mapped_column()
    vin: Mapped[str] = mapped_column(unique=True)
    last_maintenance: Mapped[str] = mapped_column()

    