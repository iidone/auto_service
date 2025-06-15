import unittest
from fastapi.testclient import TestClient
from fastapi import status
from src.main import app
from src.database import engine, Base
from src.models.maintenances import MaintenancesModel
from src.models.clients import ClientsModel
from src.models.users import UsersModel
from sqlalchemy import select, insert
import asyncio
from datetime import datetime

class TestMaintenancesEndpoints(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.client = TestClient(app)
        cls.test_maintenance_data = {
            "user_id": 1,
            "client_id": 1,
            "description": "Test maintenance",
            "date": datetime.now().isoformat(),
            "next_maintenance": datetime.now().isoformat(),
            "comment": "Test comment",
            "status": "waiting",
            "price": "1000"
        }
        
    async def async_setup(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)
            await conn.run_sync(Base.metadata.create_all)

        async with engine.begin() as conn:
            await conn.execute(
                insert(UsersModel).values(
                    id=1,
                    username="testuser",
                    password="hashedpass",
                    first_name="Test",
                    last_name="User",
                    contact="1234567890",
                    role="master"
                )
            )
            await conn.execute(
                insert(ClientsModel).values(
                    id=1,
                    first_name="Client",
                    last_name="Test",
                    contact="9876543210",
                    brand="Test Brand",
                    series="Test Series",
                    number="A123BC",
                    mileage="10000",
                    age="2",
                    vin="TESTVIN123456789",
                    last_maintenance=datetime.now().isoformat()
                )
            )
            await conn.execute(
                insert(MaintenancesModel).values(
                    **self.test_maintenance_data
                )
            )

    async def async_teardown(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)

    def setUp(self):
        asyncio.run(self.async_setup())

    def tearDown(self):
        asyncio.run(self.async_teardown())

    def test_add_maintenance(self):
        new_maintenance = {
            "user_id": 1,
            "client_id": 1,
            "description": "New maintenance",
            "date": datetime.now().isoformat(),
            "next_maintenance": datetime.now().isoformat(),
            "comment": "New comment",
            "status": "waiting",
            "price": "1500"     
        }

        response = self.client.post(
            "/maintenances/add_maintenance",
            json=new_maintenance
        )

        self.assertEqual(response.status_code, status.HTTP_201_CREATED)

        response_data = response.json()
        self.assertEqual(response_data["description"], "New maintenance")
        self.assertEqual(response_data["status"], "waiting")

    def test_get_all_maintenances(self):
        response = self.client.get("/maintenances/all_maintenances")
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 1)
        self.assertEqual(response.json()[0]["description"], "Test maintenance")

    def test_get_maintenances_with_clients(self):
        response = self.client.get("/maintenances/maintenances_with_clients")
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 1)
        self.assertEqual(response.json()[0]["maintenance"]["description"], "Test maintenance")
        self.assertEqual(response.json()[0]["client"]["first_name"], "Client")

    def test_get_maintenance_with_client_by_id(self):
        maintenances = self.client.get("/maintenances/all_maintenances").json()
        maintenance_id = maintenances[0]["id"]
        
        response = self.client.get(f"/maintenances/maintenances_with_clients/{maintenance_id}")
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(response.json()["maintenance"]["description"], "Test maintenance")
        self.assertEqual(response.json()["client"]["first_name"], "Client")

    def test_get_maintenances_by_master(self):
        response = self.client.get("/maintenances/by_master/1")
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 1)
        self.assertEqual(response.json()[0]["maintenance"]["description"], "Test maintenance")

    def test_delete_many_maintenances(self):
        maintenances = self.client.get("/maintenances/all_maintenances").json()
        maintenance_id = maintenances[0]["id"]

        response = self.client.post(
            "/maintenances/delete-many",
            json={"ids": [maintenance_id]}
        )
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(response.json()["message"], "Успешно удалено ТО: 1")

        maintenances = self.client.get("/maintenances/all_maintenances").json()
        self.assertEqual(len(maintenances), 0)

    def test_update_maintenance_status(self):
        maintenances = self.client.get("/maintenances/all_maintenances").json()
        maintenance_id = maintenances[0]["id"]
        
        update_data = {
            "status": "in_progress",
            "price": "1200"
        }
        
        response = self.client.patch(
            f"/maintenances/update_maintenance_status/{maintenance_id}",
            json=update_data
        )
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(response.json()["status"], "in_progress")
        self.assertEqual(response.json()["price"], "1200")

if __name__ == '__main__':
    unittest.main()