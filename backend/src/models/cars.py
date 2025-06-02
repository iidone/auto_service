from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class CarsModel(Base):
    __tablename__ = "cars"

    id: Mapped[int] = mapped_column(primary_key=True)
    user_id: Mapped[int] = mapped_column()
    brand: Mapped[str] = mapped_column()
    series: Mapped[str] = mapped_column()
    number: Mapped[str] = mapped_column(unique=True)
    age: Mapped[str] = mapped_column()

    