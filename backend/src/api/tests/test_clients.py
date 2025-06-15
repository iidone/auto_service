import unittest
from fastapi.testclient import TestClient
from fastapi import status
from src.main import app
from src.database import engine, Base
from src.models.clients import ClientsModel
from sqlalchemy import select, insert
import asyncio
from datetime import datetime

class TestClientsEndpoints(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.client = TestClient(app)
        cls.test_client_data = {
            "first_name": "Иван",
            "last_name": "Иванов",
            "contact": "+79001234567",
            "brand": "Toyota",
            "series": "Camry",
            "number": "А123БВ77",
            "mileage": "50000",
            "age": "5",
            "vin": "XTA210997654321",
            "last_maintenance": datetime.now().isoformat()
        }
        
    async def async_setup(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)
            await conn.run_sync(Base.metadata.create_all)

        async with engine.begin() as conn:
            await conn.execute(
                insert(ClientsModel).values(
                    **self.test_client_data
                )
            )

    async def async_teardown(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)

    def setUp(self):
        asyncio.run(self.async_setup())

    def tearDown(self):
        asyncio.run(self.async_teardown())

    def test_add_client(self):
        new_client = {
            "first_name": "Петр",
            "last_name": "Петров",
            "contact": "+79007654321",
            "brand": "Honda",
            "series": "Accord",
            "number": "В456ТУ77",
            "mileage": "30000",
            "age": "3",
            "vin": "JHMCM565572654321",
            "last_maintenance": datetime.now().isoformat()
        }
        
        response = self.client.post(
            "/clients/add_client",
            json=new_client
        )
        
        self.assertEqual(response.status_code, status.HTTP_201_CREATED)
        self.assertEqual(response.json()["first_name"], "Петр")
        self.assertEqual(response.json()["brand"], "Honda")

    def test_add_client_validation(self):
        invalid_client = {
            "first_name": "OnlyName"
        }
        
        response = self.client.post(
            "/clients/add_client",
            json=invalid_client
        )
        
        self.assertEqual(response.status_code, status.HTTP_422_UNPROCESSABLE_ENTITY)

    def test_get_all_clients(self):
        self.test_add_client()
        
        response = self.client.get("/clients/all_clients")
        
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 2)
        self.assertEqual(response.json()[0]["last_name"], "Иванов")
        self.assertEqual(response.json()[1]["last_name"], "Петров")

    def test_client_unique_constraints(self):
        duplicate_client = {
            "first_name": "Дубликат",
            "last_name": "Клиент",
            "contact": "+79000000000",
            "brand": "Duplicate",
            "series": "Model",
            "number": "А000АА77",
            "mileage": "0",
            "age": "0",
            "vin": "XTA210997654321",
            "last_maintenance": datetime.now().isoformat()
        }
        
        response = self.client.post(
            "/clients/add_client",
            json=duplicate_client
        )
        
        self.assertEqual(response.status_code, status.HTTP_500_INTERNAL_SERVER_ERROR)

if __name__ == '__main__':
    unittest.main()