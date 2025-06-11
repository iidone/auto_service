from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class SparePartsModel(Base):
    __tablename__ = "spare_parts"

    id: Mapped[int] = mapped_column(primary_key=True, autoincrement=True)
    title: Mapped[str] = mapped_column()
    category: Mapped[str] = mapped_column()
    article: Mapped[str] = mapped_column(unique=True)
    analog: Mapped[str] = mapped_column()
    supplier: Mapped[str] = mapped_column()
    price: Mapped[str] = mapped_column()
    