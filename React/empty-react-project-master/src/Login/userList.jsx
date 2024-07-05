import React, { useState, useEffect } from 'react';
import axios from 'axios';

const UserList = () => {
  const [users, setUsers] = useState([]);

  useEffect(() => {
    fetchUsers();
  }, []);
  
  const GetToken = () => {
    const token = localStorage.getItem('token');
    console.log("Token from local storage:", token); // Ensure the token is correctly retrieved
    return token;
  }

  const fetchUsers = async () => {
    const token = GetToken();
    const config = {
      headers: {
        "Authorization": `Bearer ${token}`
      }
    };

    try {
      const response = await axios.get('http://localhost:8613/Ride/statefull');
      console.log("Fetched Users:", response.data); // Ensure the response is as expected
      setUsers(response.data);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  return (
    <div>
      <h1>User List</h1>
      <ul>
        {users.map(user => (
          <li key={user.id}>{user.name}</li>
        ))}
      </ul>
    </div>
  );
};

export default UserList;
