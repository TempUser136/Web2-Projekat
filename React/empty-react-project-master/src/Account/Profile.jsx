import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import User from "../Models/User";
import { updateUserProfile } from '../api.js';
import '../Style/profile.css'

const Profile = () => {
  const [formData, setFormData] = useState({
    Username: "",
    Email: "",
    Password: "",
    Name: "",
    LastName: "",
    Birthday: "",
    Address: "",
    Type: "",
    Image: null,
  });
  const navigate = useNavigate();

  useEffect(() => {
    const storedUser = JSON.parse(localStorage.getItem("user"));
    if (storedUser) {
      setFormData({
        Username: storedUser.username,
        Email: storedUser.email,
        Password: "",
        Name: storedUser.name,
        LastName: storedUser.lastName,
        Birthday: storedUser.birthday,
        Address: storedUser.address,
        Type: storedUser.type,
        Image: storedUser.imageBase64,
      });
    }
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleFileChange = (e) => {
    setFormData({
      ...formData,
      Image: e.target.files[0],
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);

    const user = new User(
      formData.get('Username'),
      formData.get('Email'),
      formData.get('Password'),
      formData.get('Name'),
      formData.get('LastName'),
      formData.get('Birthday'),
      formData.get('Address'),
      formData.get('Type'),
      formData.get('Image')
    );

    try {
      const response = await updateUserProfile(formData,1);
      console.log('User added:', response.data);
      navigate('/');
    } catch (error) {
      console.error('There was an error adding the user!', error);
    }
  };

  const getBase64String = async (file) => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onloadend = () => resolve(reader.result);
      reader.onerror = reject;
    });
  };

  return (
    <div className="profile">
      <h1>Update Profile</h1>
      <form className="forma" onSubmit={handleSubmit}>
        <input
          type="text"
          name="Username"
          value={formData.Username}
          placeholder="Username"
          onChange={handleInputChange}
        />
        <br />
        <input
          type="email"
          name="Email"
          value={formData.Email}
          placeholder="E-mail"
          onChange={handleInputChange}
        />
        <br />
        <input
          type="password"
          name="Password"
          value={formData.Password}
          placeholder="Password"
          onChange={handleInputChange}
        />
        <br />
        <input
          type="text"
          name="Name"
          value={formData.Name}
          placeholder="Name"
          onChange={handleInputChange}
        />
        <br />
        <input
          type="text"
          name="LastName"
          value={formData.LastName}
          placeholder="Last name"
          onChange={handleInputChange}
        />
        <br />
        <label>Birthday:</label>
        <input
          type="date"
          name="Birthday"
          value={formData.Birthday}
          onChange={handleInputChange}
        />
        <br />
        <input
          type="text"
          name="Address"
          value={formData.Address}
          placeholder="Address"
          onChange={handleInputChange}
        />
        <br />
        <label>Type:</label>
        <select
          name="Type"
          value={formData.Type}
          onChange={handleInputChange}
        >
          <option value="Administrator">Administrator</option>
          <option value="User">User</option>
          <option value="Driver">Driver</option>
        </select>
        <br />
        <label>Image:</label>
        <input type="file" name="Image" onChange={handleFileChange} />
        <br />
        <input type="submit" value="Save Changes" />
      </form>
    </div>
  );
};

export default Profile;
