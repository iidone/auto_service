from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class UsersModel(Base):
    __tablename__ = "users"

    id: Mapped[int] = mapped_column(primary_key=True)
    username: Mapped[str] = mapped_column(unique=True)
    first_name: Mapped[str] = mapped_column()
    last_name: Mapped[str] = mapped_column()
    password: Mapped[str] = mapped_column()
    contact: Mapped[str] = mapped_column()
    vin: Mapped[str] = mapped_column()
    