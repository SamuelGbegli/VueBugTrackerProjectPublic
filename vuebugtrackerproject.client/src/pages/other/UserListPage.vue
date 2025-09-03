<!--Page for viewing a list of accounts-->
<!--TODO:
  Test pagination by adding 20+ accounts
-->
<template>
  <div v-if="authStore.getUserRole() != AccountRole.Normal">
    <div class="q-pa-md">
  <h4>Account list</h4>
    <div class="row">
      <h6>Total accounts: {{ !!accountContainer ? accountContainer.totalAccounts : 0 }}</h6>
    </div>
    <div v-if="accountContainer.totalAccounts > 0">
    <!--Table to show accounts-->
      <QMarkupTable>
        <thead>
          <tr>
            <th>Username</th>
            <th>Date joined</th>
            <th>Role</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="x in accountContainer.accounts" :class="x.suspended ? 'bg-red' : ''" v-bind:key="(x as AccountViewModel).id">
            <td>
              <UserIcon :username="(x as AccountViewModel).username" :icon="(x as AccountViewModel).icon"/>
            </td>
            <td>{{ formatDate((x as AccountViewModel).dateCreated) }}</td>
            <td>
            {{
              (x as AccountViewModel).role === AccountRole.SuperUser ? "Super user" :
              (x as AccountViewModel).role === AccountRole.Administrator ? "Administrator" :
              "Normal"
             }}
             </td>
            <td>
              <QBtnDropdown v-if="(x as AccountViewModel).id != authStore.getUserID() && (x as AccountViewModel).role != AccountRole.SuperUser"
              label="Actions">
                <QList>
                  <QItem clickable v-close-popup @click="toggleAccountSuspension(x)">
                    <QItemSection>
                      <QItemLabel>{{ (x as AccountViewModel).suspended ? "Unsuspend" : "Suspend" }}</QItemLabel>
                    </QItemSection>
                  </QItem>
                  <QItem v-if="!(x as AccountViewModel).suspended" v-close-popup clickable @click="openRoleDialog(x)">
                    <QItemSection>
                      <QItemLabel>Change role</QItemLabel>
                    </QItemSection>
                  </QItem>
                </QList>
              </QBtnDropdown>
            </td>
          </tr>
        </tbody>
      </QMarkupTable>
      <div class="row">
        <QSpace/>
        <QPagination :min="1" :max="accountContainer.pages"
              v-model="currentPage"
              @update:model-value="getAccounts"
              input/>
      </div>
    </div>
    <!--Message if there are no users-->
    <h5 v-if="!!accountContainer && accountContainer.totalAccounts === 0">There are no accounts.</h5>
  </div>
  <QDialog v-model="showRoleDialog" persistent>
    <QCard style="min-width: 400px;">
      <QCardSection>
        <div class="row">
          <h6>Assign role</h6>
          <QSpace/>
          <QBtn icon="close" flat round dense v-close-popup/>
        </div>
      </QCardSection>
      <QForm @submit="changeUserRole">
        <QCardSection>
          <h6>Selected user: {{ selectedAccount.username }}</h6>
          <QSelect :options="accountRoleOptions" v-model="selectedAccountRole" label="Role" stack-label/>
        </QCardSection>
        <QCardActions align="right">
          <QBtn type="submit" label="Submit"/>
        </QCardActions>
      </QForm>
    </QCard>
  </QDialog>
  </div>
  <ErrorBanner v-else
  :prompt-type="ErrorPromptType.GoBackButtonOnly"
  :message="'The page you requested does not exist.'"/>
</template>
<script setup lang="ts">
  import RoleDTO from '@/classes/DTOs/RoleDTO';
import formatDate from '@/classes/helpers/FormatDate';
import ErrorBanner from '@/components/ErrorBanner.vue';
  import UserIcon from '@/components/UserIcon.vue';
  import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';
  import AccountRole from '@/enumConsts/Role';
  import { useAuthStore } from '@/stores/AuthStore';
  import AccountContainer from '@/viewmodels/AccountContainer';
  import type AccountViewModel from '@/viewmodels/AccountViewModel';
  import axios from 'axios';
  import { Dialog, Loading, Notify } from 'quasar';
  import { onBeforeMount, ref } from 'vue';

  //The number of pages of accounts, in groups of up to 20
  //const numberOfPages = ref(1);

  //The current page the user is on
  const currentPage = ref(1);

  //Container to store account information
  const accountContainer = ref(new AccountContainer());

  //If true, shows a loading animation on the account list
  const loading = ref(false);

  //If true, shows a dialog to edit a user's role
  const showRoleDialog = ref(false);

  //Stores user role options
  const accountRoleOptions = ["Normal", "Administrator"];

  //Stores selected user role option
  const selectedAccountRole = ref("Normal");

  //Stores the user whose role is being edited
  const selectedAccount = ref();

  const authStore = useAuthStore();

  onBeforeMount(async () => {
    await getAccounts();
  });

  //Gets accounts from the server
  async function getAccounts() {
    loading.value = true;
    try {
      const response = await axios.get(`/accounts/get?pageNumber=${currentPage.value}`);
      accountContainer.value = Object.assign(new AccountContainer(), response.data);
    } catch {

    }
    loading.value = false;
  }

  //Opens the user role dialog
  function openRoleDialog(account: AccountViewModel){
    selectedAccount.value = account;
    selectedAccountRole.value = accountRoleOptions[account.role];
    showRoleDialog.value = true;
  }

  //Suspends or unsuspends a user's account
  async function toggleAccountSuspension(account: AccountViewModel) {
    Dialog.create({
      component: ConfirmationDialog,
      componentProps: {
        header: account.suspended ? "Unsuspend user" : "Suspend user",
        message: `Are you sure you want to ${account.suspended ? "unsuspend" : "suspend"} ${account.username}?`
      }
    }).onOk(async () => {
      Loading.show({
        message: "Please wait..."
      });
      try{
        await axios.patch("accounts/suspend", account.id,{
          headers: {"Content-Type": "application/json"}
        });
        await getAccounts();
        Notify.create({
          message: `Successfully ${account.suspended ? "unsuspended" : "suspended"} ${account.username}.`,
          position: "bottom",
          type: "positive"
        });
      }
      catch{
        Notify.create({
          message: "Something went wrong when processing your request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
      }
      Loading.hide();
    });
  }

  //Changes the role of a user
  async function changeUserRole() {

    Loading.show({
        message: "Please wait..."
      });

    const roleDTO = new RoleDTO();
    roleDTO.accountID = selectedAccount.value.id;
    roleDTO.role = accountRoleOptions.indexOf(selectedAccountRole.value);

    try {
      await axios.patch("accounts/updaterole", roleDTO);
      await getAccounts();
      showRoleDialog.value = false;
      Notify.create({
          message: `Successfully updated ${selectedAccount.value.username}'s role.`,
          position: "bottom",
          type: "positive"
        });
    } catch {
      Notify.create({
          message: "Something went wrong when processing your request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
    }

    Loading.hide();
  }

</script>
