import React, { useEffect, useState } from 'react';
import UserAcc from '../Account/User';
import Driver from '../Account/Driver';

function Home() {
  const [user, setUser] = useState(null);

  useEffect(() => {
    // Retrieve user data from localStorage
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser);
        setUser(parsedUser);
      } catch (error) {
        console.error('Error parsing user data from localStorage', error);
      }
    }
  }, []);

  if (!user) {
    return <div>No user data available. Please log in.</div>;
  }

  if (user.type === "User") {
    return (
      <>
        <div>
          <h1>Profile</h1>
          <p>Username: {user.username}</p>
          <p>Email: {user.email}</p>
          <p>Name: {user.name}</p>
          <p>Last Name: {user.lastName}</p>
          {user.imageBase64 && (
            <img src={`data:image/png;base64,${user.image}`} alt="User Profile" />
          )}
          {/* Add other user data as needed */}
        </div>
        <UserAcc />
      </>
    );
  }

  if (user.type === "Driver") {
    return <Driver />;
  }

  return (
    <div>
      <h1>Profile</h1>
      <p>Username: {user.username}</p>
      <p>Email: {user.email}</p>
      <p>Name: {user.name}</p>
      <p>Last Name: {user.lastName}</p>
      {user.imageBase64 && (
        <img src={`data:image/png;base64,${user.image}`} alt="User Profile" />
      )}
      {/* Add other user data as needed */}
    </div>
  );
}

export default Home;
