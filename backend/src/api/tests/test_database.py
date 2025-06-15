import unittest
from fastapi.testclient import TestClient
from src.main import app
from src.database import engine, Base
from src.models.users import UsersModel
from src.models.spare_parts import SparePartsModel
from src.models.maintenances import MaintenancesModel
from src.models.clients import ClientsModel
from src.models.checks import ChecksModel
import asyncio

class TestDatabaseSetup(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.client = TestClient(app)
        
    async def async_drop_all(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)
    
    def test_setup_db(self):
        asyncio.get_event_loop().run_until_complete(self.async_drop_all())

        async def check_tables_dropped():
            async with engine.connect() as conn:
                tables = await conn.run_sync(
                    lambda sync_conn: Base.metadata.tables.keys()
                )
                return tables
                
        tables_before = asyncio.get_event_loop().run_until_complete(check_tables_dropped())
        self.assertEqual(len(tables_before), 0, "Tables should be empty before setup")

        response = self.client.post("/database/setup_db")
        
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json(), {"message": "Database setup successfully"})

        async def check_tables_created():
            async with engine.connect() as conn:
                inspector = await conn.run_sync(
                    lambda sync_conn: inspect(sync_conn)
                )
                return inspector.get_table_names()
                
        tables_after = asyncio.get_event_loop().run_until_complete(check_tables_created())
        expected_tables = {
            'users', 'spare_parts', 'maintenances', 
            'clients', 'checks'
        }
        self.assertTrue(expected_tables.issubset(tables_after), "Not all tables were created")

if __name__ == '__main__':
    unittest.main()