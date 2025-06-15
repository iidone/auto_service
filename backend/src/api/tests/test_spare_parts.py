import unittest
from fastapi.testclient import TestClient
from fastapi import status
from src.main import app
from src.database import engine, Base
from src.models.spare_parts import SparePartsModel
from sqlalchemy import select, insert
import asyncio

class TestSparePartsEndpoints(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.client = TestClient(app)
        cls.test_part_data = {
            "title": "Тестовая запчасть",
            "category": "Двигатель",
            "article": "TEST-001",
            "analog": "ANALOG-001",
            "supplier": "Поставщик",
            "price": "1000.50"
        }
        
    async def async_setup(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)
            await conn.run_sync(Base.metadata.create_all)
        
        # Create test spare part
        async with engine.begin() as conn:
            await conn.execute(
                insert(SparePartsModel).values(
                    **self.test_part_data
                )
            )

    async def async_teardown(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)

    def setUp(self):
        asyncio.run(self.async_setup())

    def tearDown(self):
        asyncio.run(self.async_teardown())

    def test_add_spare_part(self):
        new_part = {
            "title": "Новая запчасть",
            "category": "Тормоза",
            "article": "NEW-001",
            "analog": "NEW-ANALOG",
            "supplier": "Новый поставщик",
            "price": "2000.00"
        }
        
        response = self.client.post(
            "/spare-parts/add_spare_parts",
            json=new_part
        )
        
        self.assertEqual(response.status_code, status.HTTP_201_CREATED)
        self.assertEqual(response.json()["title"], "Новая запчасть")
        self.assertEqual(response.json()["article"], "NEW-001")

    def test_add_spare_part_validation(self):
        # Тест с неполными данными
        invalid_part = {
            "title": "Только название"  # Отсутствуют другие обязательные поля
        }
        
        response = self.client.post(
            "/spare-parts/add_spare_parts",
            json=invalid_part
        )
        
        self.assertEqual(response.status_code, status.HTTP_422_UNPROCESSABLE_ENTITY)

    def test_get_all_spare_parts(self):
        response = self.client.get("/spare-parts/all_spare_parts")
        
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 1)
        self.assertEqual(response.json()[0]["title"], "Тестовая запчасть")

    def test_delete_spare_parts(self):
        # Сначала получаем ID существующей запчасти
        parts = self.client.get("/spare-parts/all_spare_parts").json()
        part_id = parts[0]["id"]
        
        # Тест успешного удаления
        response = self.client.post(
            "/spare-parts/delete-many",
            json={"ids": [part_id]}
        )
        
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(response.json()["message"], "Успешно удалено запчастей: 1")
        
        # Проверяем что запчасть действительно удалена
        parts = self.client.get("/spare-parts/all_spare_parts").json()
        self.assertEqual(len(parts), 0)

    def test_delete_spare_parts_validation(self):
        # Тест пустого списка ID
        response = self.client.post(
            "/spare-parts/delete-many",
            json={"ids": []}
        )
        self.assertEqual(response.status_code, status.HTTP_400_BAD_REQUEST)
        
        # Тест несуществующих ID
        response = self.client.post(
            "/spare-parts/delete-many",
            json={"ids": [999]}
        )
        self.assertEqual(response.status_code, status.HTTP_404_NOT_FOUND)

    def test_unique_article_constraint(self):
        # Пытаемся добавить запчасть с существующим артикулом
        duplicate_part = {
            "title": "Дубликат",
            "category": "Дубликат",
            "article": self.test_part_data["article"],  # Используем существующий артикул
            "analog": "DUPLICATE",
            "supplier": "DUPLICATE",
            "price": "0"
        }
        
        response = self.client.post(
            "/spare-parts/add_spare_parts",
            json=duplicate_part
        )
        
        # Ожидаем ошибку (400 или 500 в зависимости от реализации)
        self.assertIn(response.status_code, 
                     [status.HTTP_400_BAD_REQUEST, 
                      status.HTTP_500_INTERNAL_SERVER_ERROR])

if __name__ == '__main__':
    unittest.main()