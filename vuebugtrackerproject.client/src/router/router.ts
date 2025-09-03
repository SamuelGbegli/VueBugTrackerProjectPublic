import AccountRecoveryPage from "@/pages/auth/AccountRecoveryPage.vue";
import PasswordResetPage from "@/pages/auth/PasswordResetPage.vue";
import RegistrationPage from "@/pages/auth/RegistrationPage.vue";
import BrowsePage from "@/pages/BrowsePage.vue";
import BugDiscussionPage from "@/pages/bugs/BugDiscussionPage.vue";
import BugSettingsPage from "@/pages/bugs/BugSettingsPage.vue";
import ViewBugPage from "@/pages/bugs/ViewBugPage.vue";
import CreditsPage from "@/pages/CreditsPage.vue";
import MainPage from "@/pages/MainPage.vue";
import NotFoundPage from "@/pages/other/NotFoundPage.vue";
import Sandbox from "@/pages/other/sandbox.vue";
import UserListPage from "@/pages/other/UserListPage.vue";
import AddBugPage from "@/pages/projects/AddBugPage.vue";
import CreateProjectPage from "@/pages/projects/CreateProjectPage.vue";
import ProjectBugsPage from "@/pages/projects/ProjectBugsPage.vue";
import ProjectSettingsPage from "@/pages/projects/ProjectSettingsPage.vue";
import ViewProjectPage from "@/pages/projects/ViewProjectPage.vue";
import SearchPage from "@/pages/SearchPage.vue";
import BugTemplate from "@/templates/BugTemplate.vue";
import MainTemplate from "@/templates/MainTemplate.vue";
import ProjectTemplate from "@/templates/ProjectTemplate.vue";

import { createRouter, createWebHistory } from "vue-router"

import UserSettingsPage from "@/pages/other/UserSettingsPage.vue";

const router = createRouter({
 history: createWebHistory(),
 routes: [
   {
    path: "/",
    component: MainTemplate,
    children:[
      {
        path: "",
        component: MainPage
      },
      {
        path: "register",
        component: RegistrationPage
      },
      {
        path: "recoveraccount",
        component: AccountRecoveryPage
      },
      {
        path: "resetpassword",
        component: PasswordResetPage
      },
      {
        path: "browse/",
        component: BrowsePage
      },
      {
        path: "browse/:page",
        component: BrowsePage
      },
      {
        path: "search",
        component: SearchPage
      },
      {
        path: "createproject",
        component: CreateProjectPage
      },
      {
        path: "userlist",
        component: UserListPage
      },
      {
        path: "credits",
        component: CreditsPage
      },
      //{
      //  path: "sandbox",
      //  component: Sandbox
      //},
      {
        path: "settings",
        component: UserSettingsPage
      },
      {
        path: "project",
        component: ProjectTemplate,
        children:[
          {
            path: ":projectId",
            component: ViewProjectPage
          },
          {
            path: ":projectId/bugs",
            component: ProjectBugsPage
          },
          {
            path: ":projectId/bugs/add",
            component: AddBugPage
          },
          {
            path: ":projectId/settings",
            component: ProjectSettingsPage
          }
        ]
      },
      {
        path: "bug",
        component:BugTemplate,
        children:[
          {
            path: ":bugId",
            component: ViewBugPage
          },
          {
            path: ":bugId/settings",
            component: BugSettingsPage
          },
          {
            path: ":bugId/discussion",
            component: BugDiscussionPage
          }
        ]
      },
      {
        path: "/:pathMatch(.*)*",
        component: NotFoundPage
      }
    ]
  }
 ]
})

export default router;
