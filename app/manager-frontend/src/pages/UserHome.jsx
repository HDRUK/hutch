import { HStack, Text, Stack } from "@chakra-ui/react";
import { ActionCard } from "components/ActionCard";
import { ActivitySourcesList } from "./ActivitySource/list";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { AgentsList } from "./Agent/list";
import { UsersList } from "./User/list";

export const UserHome = () => {
  const homepageActions = [
    {
      title: "Activity Source",
      description: "Activity Source action card description",
      href: "/activitysourcelist",
    },
    {
      title: "Agents",
      description: "Agents action card description",
      href: "/agentlist",
    },
    {
      title: "Users",
      description: "Users action card description",
      href: "/userlist",
    },
  ];

  const { listname } = useParams(); // grab listname from url
  const navigate = useNavigate();

  useEffect(() => {
    if (!listname) navigate("/home/activitysourcelist"); // load activitysourcelist if params not available
  }, []);

  const List = () => {
    // conditional loading of the list based on listname
    switch (listname) {
      case "agentlist":
        return <AgentsList />; // load Agent list if listname(params) is 'agentlist'
      case "userlist":
        return <UsersList />; // load User list if listname(params) is 'userlist'
      default:
        return <ActivitySourcesList />; // load Activity Source list if listname(params) is 'activitysourcelist'
    }
  };

  return (
    <Stack px={8} w="100%" spacing={4} p={4} alignItems="center">
      <HStack
        maxW="800"
        w="100%"
        justifyContent="center"
        spacing={5}
        pb={3.5}
        mb={2}
        borderBottomColor="blue.300"
        borderBottomWidth={2}
        borderBottomRadius={5}
      >
        {homepageActions.map((action, index) => (
          <ActionCard
            key={index}
            title={action.title}
            href={`/home${action.href}`}
            isActive={listname === action.href.replace("/", "")}
          >
            <Text color="gray.600" fontSize="sm">
              {action.description}
            </Text>
          </ActionCard>
        ))}
      </HStack>
      <List />
    </Stack>
  );
};
