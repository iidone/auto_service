from sqlalchemy import select
from typing_extensions import Annotated
from fastapi import Depends, HTTPException, status
from sqlalchemy.ext.asyncio import AsyncSession
from src.database import get_session
from passlib.context import CryptContext
from fastapi.security import OAuth2PasswordBearer
from jose import JWTError, jwt
from datetime import datetime, timedelta
import os
from dotenv import load_dotenv
from pathlib import Path

env_path = Path(__file__).parent.parent.parent / ".env"
load_dotenv(env_path)

SessionDep = Annotated[AsyncSession, Depends(get_session)]

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

def verify_password(plain_password: str, hashed_password: str):
    return pwd_context.verify(plain_password, hashed_password)

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="users/login")

SECRET_KEY = os.getenv("SECRET_KEY")
if not SECRET_KEY:
    raise ValueError("SECRET_KEY environment variable must be set")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30

token_blacklist = {}



def create_access_token(data: dict):
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    to_encode.update({"exp": expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

async def get_user_by_username(username: str, session: AsyncSession):
    from src.models.users import UsersModel
    result = await session.execute(
        select(UsersModel).where(UsersModel.username == username)
    )
    return result.scalar_one_or_none()

async def authenticate_user(username: str, password: str, session: AsyncSession):
    user = await get_user_by_username(username, session)
    if not user:
        return False
    if not verify_password(password, user.password):
        return False
    return user

async def get_current_user(session: SessionDep, token: str = Depends(oauth2_scheme)):
    clean_blacklist()
    
    if token in token_blacklist:
        raise HTTPException(status_code=401, detail="Token revoked")
    
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        username: str = payload.get("sub")
        if username is None:
            raise HTTPException(status_code=401, detail="Invalid token")
    except JWTError:
        raise HTTPException(status_code=401, detail="Invalid token")
    
    user = await get_user_by_username(username, session)
    if user is None:
        raise HTTPException(status_code=401, detail="User not found")
    return user

async def add_to_blacklist(token: str):
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM], options={"verify_exp": False})
        expiry = datetime.fromtimestamp(payload["exp"])
        token_blacklist[token] = expiry
    except JWTError:
        pass

def clean_blacklist():
    now = datetime.now()
    expired_tokens = [token for token, expiry in token_blacklist.items() if expiry < now]
    for token in expired_tokens:
        token_blacklist.pop(token)