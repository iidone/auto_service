from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class ChecksModel(Base):
    __tablename__ = "checks"

    id: Mapped[int] = mapped_column(primary_key=True)
    maintenance_id: Mapped[int] = mapped_column()
    spare_parts_id: Mapped[int] = mapped_column()
    total_price: Mapped[str] = mapped_column()


    