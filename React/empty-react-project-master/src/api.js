import axios from 'axios';

// Fetch API base URL from the environment variables
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

export const loginUser = async (loginModel) => {
  return await axios.post(`${API_BASE_URL}/user/Login/1`, loginModel);
};

export const googleLogin = async (tokenId) => {
  return await axios.post(`${API_BASE_URL}/user/google-login`, { token: tokenId });
};

// Fetch all drivers
export const fetchDrivers = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/user/GetDrivers`);
    return response.data;
  } catch (err) {
    throw new Error('Error fetching drivers');
  }
};
export const fetchDriverStatus = async (username) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/user/GetDriverStatus`, {
      params: { username,id:1 }
    });
    return response.data;
  } catch (err) {
    throw new Error('Error fetching driver status');
  }
};

export const fetchUnapproved = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/user/GetUnapproved`);
    return response.data;
  } catch (err) {
    throw new Error('Error fetching drivers');
  }
};

// Fetch all rides
export const fetchRides = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/ride/GetAllRides`);
    return Array.isArray(response.data) ? response.data : [];
  } catch (err) {
    throw new Error('Error fetching rides');
  }
};

// Approve driver
export const approveDriver = async (username) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/user/ApproveDriver`, { username });
    return response.status === 200;
  } catch (err) {
    throw new Error('Error approving driver');
  }
};

// Decline driver
export const declineDriver = async (username) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/user/DeclineDriver`, { username });
    return response.status === 200;
  } catch (err) {
    throw new Error('Error declining driver');
  }
};

// Block driver
export const blockDriver = async (username) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/user/BlockDriver`, { username });
    return response.status === 200;
  } catch (err) {
    throw new Error('Error blocking driver');
  }
};

// Unblock driver
export const unblockDriver = async (username) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/user/UnblockDriver`, { username });
    return response.status === 200;
  } catch (err) {
    throw new Error('Error unblocking driver');
  }
};

// Update ride status
export const updateRideStatus = async (id, newStatus) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/ride/UpdateRideStatus`, { id, status: newStatus });
    return response.status === 200;
  } catch (err) {
    throw new Error('Error updating ride status');
  }
};

// Update user profile
export const updateUserProfile = async (formData) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/user/UpdateUser`, formData);
    return response.data;
  } catch (err) {
    throw new Error('Error updating user profile');
  }
};

