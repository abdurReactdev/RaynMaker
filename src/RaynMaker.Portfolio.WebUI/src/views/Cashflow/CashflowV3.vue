<template>
    <div>
        <CCard>
            <CCardHeader>
                <CCardTitle>Cashflow</CCardTitle>
            </CCardHeader>
            <CCardBody>
                <label>Limit: </label>
                <input label="Limit" v-model="limit" @keyup.enter="onLimitChanged">

                <table class="table-hover">
                    <thead>
                        <tr>
                            <th class="date">Date</th>
                            <th class="comment">Type/Comment</th>
                            <th class="value">Value</th>
                            <th class="value">Balance</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="t in transactions" style="border-top-style:solid;border-top-width:1px"
                            :key="t.date + Math.random() ">
                            <td class="date">
                                {{ t.date }}
                            </td>
                            <td class="comment" style="padding-left:100px;padding-right:100px;">
                                <b>{{ t.type }}</b><br />
                                {{ t.comment }}
                            </td>
                            <td class="value">
                                {{ t.value }}
                            </td>
                            <td class="value">
                                {{ t.balance }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </CCardBody>
        </CCard>
    </div>
</template>

<script setup>
import API from '@/api'
import { ref, watchEffect } from 'vue'

let transactions = ref(null)
const limit = ref(25)
watchEffect(async () => {
    console.log('watching', limit.value)
    const response = await API.get(`/cashflow?limit=${limit.value}`)

    transactions = response.data
})
</script>
