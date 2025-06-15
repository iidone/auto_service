import unittest
from fastapi.testclient import TestClient
from fastapi import status
from src.main import app
from src.database import engine, Base
from src.models.users import UsersModel
from sqlalchemy import select, insert
import asyncio
from passlib.context import CryptContext

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

class TestUsersEndpoints(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.client = TestClient(app)
        cls.test_user_data = {
            "username": "testuser",
            "password": "testpass",
            "first_name": "Test",
            "last_name": "User",
            "contact": "1234567890",
            "role": "master"
        }
        cls.hashed_password = pwd_context.hash("testpass")
        
    async def async_setup(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)
            await conn.run_sync(Base.metadata.create_all)

        async with engine.begin() as conn:
            await conn.execute(
                insert(UsersModel).values(
                    username=self.test_user_data["username"],
                    password=self.hashed_password,
                    first_name=self.test_user_data["first_name"],
                    last_name=self.test_user_data["last_name"],
                    contact=self.test_user_data["contact"],
                    role=self.test_user_data["role"]
                )
            )

    async def async_teardown(self):
        async with engine.begin() as conn:
            await conn.run_sync(Base.metadata.drop_all)

    def setUp(self):
        asyncio.run(self.async_setup())

    def tearDown(self):
        asyncio.run(self.async_teardown())

    def get_auth_headers(self):
        response = self.client.post(
            "/users/login",
            data={
                "username": self.test_user_data["username"],
                "password": "testpass"
            },
            headers={"Content-Type": "application/x-www-form-urlencoded"}
        )
        if response.status_code != 200:
            raise ValueError(f"Login failed: {response.text}")
        return {"Authorization": f"Bearer {response.json()['access_token']}"}

    def test_login_user(self):
        response = self.client.post(
            "/users/login",
            data={
                "username": self.test_user_data["username"],
                "password": "testpass"
            },
            headers={"Content-Type": "application/x-www-form-urlencoded"}
        )
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertIn("access_token", response.json())

        response = self.client.post(
            "/users/login",
            data={
                "username": "wrong",
                "password": "wrong"
            },
            headers={"Content-Type": "application/x-www-form-urlencoded"}
        )
        self.assertEqual(response.status_code, status.HTTP_401_UNAUTHORIZED)

    def test_logout_user(self):
        try:
            headers = self.get_auth_headers()
            response = self.client.post("/users/logout", headers=headers)
            self.assertEqual(response.status_code, status.HTTP_200_OK)
        except ValueError as e:
            self.fail(f"Authentication failed: {str(e)}")

    def test_add_user(self):
        try:
            headers = self.get_auth_headers()
            new_user = {
                "username": "newuser",
                "password": "newpass",
                "first_name": "New",
                "last_name": "User",
                "contact": "9876543210",
                "role": "manager"
            }
            
            response = self.client.post(
                "/users/add_user",
                json=new_user,
                headers=headers
            )
            self.assertEqual(response.status_code, status.HTTP_201_CREATED)
        except ValueError as e:
            self.fail(f"Authentication failed: {str(e)}")

    def test_get_all_users(self):
        headers = self.get_auth_headers()
        response = self.client.get("/users/all_users", headers=headers)
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 1)

    def test_get_all_masters(self):
        headers = self.get_auth_headers()
        response = self.client.get("/users/all_masters", headers=headers)
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 1)

    def test_get_all_managers(self):
        headers = self.get_auth_headers()
        response = self.client.get("/users/all_managers", headers=headers)
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertEqual(len(response.json()), 0)

    def test_delete_many_users(self):
        try:
            headers = self.get_auth_headers()

            users_response = self.client.get("/users/all_users", headers=headers)
            self.assertEqual(users_response.status_code, status.HTTP_200_OK)
            existing_users = users_response.json()
            self.assertTrue(len(existing_users) > 0, "No users found in database")
            user_id = existing_users[0]["id"]

            response = self.client.post(
                "/users/delete-many",
                json={"ids": [user_id]},
                headers=headers
            )
            self.assertEqual(response.status_code, status.HTTP_200_OK)
            self.assertEqual(response.json()["message"], "Удалено мастеров: 1")

            users_response = self.client.get("/users/all_users", headers=headers)
            self.assertEqual(len(users_response.json()), 0)

            response = self.client.post(
                "/users/delete-many",
                json={"ids": []},
                headers=headers
            )
            self.assertEqual(response.status_code, status.HTTP_400_BAD_REQUEST)

            response = self.client.post(
                "/users/delete-many",
                json={"ids": [999]},
                headers=headers
            )
            self.assertEqual(response.status_code, status.HTTP_404_NOT_FOUND)

        except ValueError as e:
            self.fail(f"Authentication failed: {str(e)}")

if __name__ == '__main__':
    unittest.main()