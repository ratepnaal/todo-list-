import { useState, useEffect } from "react";
import axios from "axios";

// يفضل ال   url يكون بمتحول لوحدو لتفادي الاخطاء وما تكتبو مليون مرة 

const API_URL = "http://localhost:5129/api/todos";


export default function ToDoList() {
  const [input, setInput] = useState("");
  const [array, setArray] = useState([]);
  const  [filteredTasks, setFilteredTasks] = useState("all");
  const [editTaskText, setEditTaskText] = useState("");
const [editingId, setEditingId] = useState(null); // الاعتماد على ID وليس Index
  // جلب المهام من الباك اند عند التحميل

  // استخدم تراي وكاتش لتعرف شو سبب الخطأ وطلع الفانكشن برا ليصير الكود ابسط 
  useEffect(() => {
    FetchTask();
  }, []);

  async function FetchTask() {
    try{
const res = await axios.get(API_URL);
    setArray(res.data);
    }
    catch(err){
console.error("Failed to fetch" , err)
    }
  }

//async مهمة بكل فانكشن الو علاقة باستيراد ال api 

async function add() {
  if (!input.trim()) return;
try {
      const res = await axios.post(API_URL, { text: input, done: false });
      setArray([...array, res.data]);
      setInput(""); // التصفير فقط عند النجاح
    } catch (err) {
      alert("Failed Adding");
    }
  };

const deleteTask = async (id) => {
    try {
      await axios.delete(`${API_URL}/${id}`);
      setArray(array.filter((t) => t.id !== id));
    } catch (err) {
      console.error(err);
    }
  };

  const edit = (task) => {
    setEditingId(task.id);
    setEditTaskText(task.text);
  };

  const toggleDone = async (task) => {
    try {
      const res = await axios.put(`${API_URL}/${task.id}`, { ...task, done: !task.done });
      setArray(array.map((t) => (t.id === task.id ? res.data : t)));
    } catch (err) {
      console.error(err);
    }
  };

const save = async (id) => {
    if (!editTaskText.trim()) return;
    const task = array.find((t) => t.id === id);
    try {
      const res = await axios.put(`${API_URL}/${id}`, { ...task, text: editTaskText });
      setArray(array.map((t) => (t.id === id ? res.data : t)));
      setEditingId(null);
    } catch (err) {
      console.error(err);
    }
  };

  // مافي داعي لفانكشن الكانسل هون مجرد ما تصفر حالة تعديل الاي دي بيرجع كلشي 

const filteredArray = array.filter((t) => {
    if (filteredTasks === "done") return t.done;
    if (filteredTasks === "active") return !t.done;
    return true;
  });

  return (
// padding يعني شوية فراغات بين العناصر 

    <div style={{padding: "20px "}}>
      <h1>Tasks!</h1>
      <button onClick={() => setFilteredTasks("all")} style={{ fontSize: "25px" }}>all</button>
      <button onClick={() => setFilteredTasks("active")} style={{ fontSize: "25px" }}>Active</button>
      <button onClick={() => setFilteredTasks("done")} style={{ fontSize: "25px" }}>Done</button>
      <br />
      <input value={input} onChange={(e) => setInput(e.target.value)} style={{ fontSize: "25px" }} placeholder="enter Task!" />
      <button onClick={add} style={{ fontSize: "25px" }}>add</button>
      {
        filteredArray.map((task) =>
          <div key={task.id}
          style={{ marginBottom: "10px", display: "flex", alignItems: "center" }}
          >
            {editingId === task.id  ?
              (<div>
                <input value={editTaskText} onChange={(e) => setEditTaskText(e.target.value)} style={{ fontSize: "25px" }} />

              {/*لا تنسى لازم ترجع الفانكشن بدالة مو لحالها متل السي شارب */}
                <button onClick={()=>save(task.id)} style={{ fontSize: "25px" }}>save</button>
                <button onClick={() => setEditingId(null)} style={{ fontSize: "25px" }}>cancel</button>
              </div>)
              :
              (<div>
                <span style={{
                  textDecoration: task.done ? "line-through" : "none",
                  color: task.done ? "green" : "#333",
                }}
                onClick={() => toggleDone(task)}>
                  {task.text}
                  </span>                
                <button onClick={() => edit(task)} style={{ fontSize: "25px" }}>edit</button>
                <button onClick={() => deleteTask(task.id)} style={{ fontSize: "25px" }}>delete</button>
              </div>)}
          </div>
        )
      }
    </div>
  );
}