import React from 'react';
import User from "../Account/User";
import UserAcc from '../Account/User';
import Driver from '../Account/Driver';
function Home() {
  const user = JSON.parse(localStorage.getItem('user'));
  if (!user) {
    return <div>No user data available. Please log in.</div>;
  }
  if(user.type == "User"){
    return <><UserAcc></UserAcc></>
  }
  if(user.type == "Driver"){
    return <><Driver></Driver></>
  }

  

  return (
    <>
    <div>
      <h1>Profile</h1>
      <p>Username: {user.username}</p>
      <p>Email: {user.email}</p>
      <p>Name: {user.name}</p>
      <p>Last Name: {user.lastName}</p>
      
      {/* Add other user data as needed */}
    </div>
    </>
  );
}

export default Home;
