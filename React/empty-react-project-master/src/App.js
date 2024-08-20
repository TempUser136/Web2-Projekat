import React from 'react';
import './App.css';
import Nesto from "./Login/SignUp"; 
import Home from "./Account/Home";
import Login from "./Login/LogIn";
import "./Login/SignUp.css";
import SignUp from './Login/SignUp';
import {BrowserRouter, Routes, Route, Link} from 'react-router-dom';
import UserList from './Login/userList';
import Profile from './Account/Profile';

function App() {
  return (
    <>

   <BrowserRouter>
    <Routes>
      <Route path="/" element={<Login/>}/>
      <Route path="/SignUp" element={<SignUp/>}/>
      <Route path="/List" element={<UserList/>}/>
      <Route path="/Home" element={<Home/>}/>
      <Route path="/Profile" element={<Profile/>}/>
    </Routes>

   </BrowserRouter>
    
    </>
  );
}
export default App;
