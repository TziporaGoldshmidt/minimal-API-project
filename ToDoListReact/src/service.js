import axios from 'axios';

// הגדרת כתובת ה-API ב-default
const apiUrl = "http://localhost:5171";
axios.defaults.baseURL = apiUrl;

// הוספת interceptor לתפיסת שגיאות
axios.interceptors.response.use(
  response => response, // מחזיר את התגובה אם אין שגיאה
  error => {
    console.error('Error response:', error.response); // רושם את השגיאה ללוג
    return Promise.reject(error); // מחזיר את השגיאה כדי שתוכל לטפל בה בהמשך
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`${apiUrl}/tasks`)    
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', name)
      try {
        const newItem = {
          Name: name,
          IsComplete: false 
      };
        const result = await axios.post(`${apiUrl}/tasks`, newItem);
        return result.data; // מחזיר את הנתונים שהתקבלו מהשרת
      } catch (error) {
        console.error('Error adding task:', error);
        throw error; // ניתן להשליך שגיאה במקרה של כישלון
      }
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    try {
      const result = await axios.put(`${apiUrl}/tasks/${id}`, { isComplete });
      return result.data; // מחזיר את הנתונים שהתקבלו מהשרת
    } catch (error) {
      console.error('Error updating task:', error);
      throw error; // ניתן להשליך שגיאה במקרה של כישלון
    }
  },

  deleteTask:async(id)=>{
    console.log('deleteTask')
    try {
      await axios.delete(`${apiUrl}/tasks/${id}`);
      return { success: true }; // מחזיר אובייקט שמעיד על הצלחה
    } catch (error) {
      console.error('Error deleting task:', error);
      throw error; // ניתן להשליך שגיאה במקרה של כישלון
    }
  }
};
