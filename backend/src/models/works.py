from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from src.database import Base


class WorksModel(Base):
    __tablename__ = "works"

    id: Mapped[int] = mapped_column(primary_key=True)
    work_type: Mapped[str] = mapped_column()
    date: Mapped[str] = mapped_column()
    deadline: Mapped[str] = mapped_column()
    next_maintenance: Mapped[str] = mapped_column()
    comment: Mapped[str] = mapped_column()




