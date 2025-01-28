<script setup>
import {ref, computed} from 'vue';

const searchQuery = ref("")
const users = ref([{ name: 'John' }, { name: 'Jane' }, { name: 'Doe' }])

const filteredUsers = computed(()=>{
  return users.value.filter((user) => user.name.toLowerCase().includes(searchQuery.value.toLowerCase()))
})

const resetHandler = () => {
  searchQuery.value = ""
}

const sortUsers = (sortKey) => { 
  users.value.sort((a, b) => {
    if(sortKey === 'asc'){
      return a.name.localeCompare(b.name)
    } else if(sortKey === 'desc'){
      return b.name.localeCompare(a.name)
    }
  })
  }

</script>

<template>
    <h1> Users</h1>
    <label for="searchQuery">Filter By Name</label>
    
    <br>
    
    <input type='text' id="searchQuery" v-model="searchQuery" placeholder="Enter query to filter"/>
    <button @click="resetHandler">
      Reset
    </button>

    <br>

    <button @click="sortUsers('asc')">
      Sort A-Z
    </button>
     <button @click="sortUsers('desc')">
      Sort Z-A
    </button>
    <ul>
      <li v-for="user in filteredUsers" :key="user.name">{{user.name}}</li>
    </ul>
</template>

<style scoped>
h1{
  text-align: center;
}
input {
  margin-right: 10px;
  padding: 5px;
}

button{
  margin: 10px;
  padding: 5px;
  background: blue;
  color: white;
  border: 0;
}
ul{
  list-style: none;
  padding: 0;
}
li{
  border: 1px black solid;
  padding: 10px;
  margin-bottom: 5px;
   background: blue;
  color: white;
}
</style>