import unittest
from fastapi.testclient import TestClient
from src.main import app
from src.database import engine, Base
from sqlalchemy import inspect
import asyncio

class TestDatabaseSetup(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.client = TestClient(app)
        
    async def async_drop_all(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)
    
    async def async_setup_all(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.create_all)
    
    async def async_get_table_names(self):
        async with engine.connect() as conn:
            return await conn.run_sync(
                lambda sync_conn: inspect(sync_conn).get_table_names()
            )
    
    def test_setup_db(self):
        asyncio.run(self.async_drop_all())

        tables_before = asyncio.run(self.async_get_table_names())
        self.assertEqual(len(tables_before), 0, "Tables should be empty before setup")

        response = self.client.post("/database/setup_db")
        self.assertEqual(response.status_code, 200)

        tables_after = asyncio.run(self.async_get_table_names())
        expected_tables = {'users', 'spare_parts', 'maintenances', 'clients', 'checks'}
        self.assertTrue(
            expected_tables.issubset(tables_after),
            f"Missing tables. Expected: {expected_tables}, got: {tables_after}"
        )

if __name__ == '__main__':
    unittest.main()