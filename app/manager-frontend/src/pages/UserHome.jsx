import { HStack, Text, Stack } from "@chakra-ui/react";
import { ActionCard } from "components/ActionCard";
import { useTranslation } from "react-i18next";
import { ActivitySourceList } from "./ActivitySource/list";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { AgentList } from "./Agent/list";

export const UserHome = () => {
  const { t, i18n, ready } = useTranslation();
  const homepageActions = t("homepageActions", { returnObjects: true }); // get homepage actions item
  const { listname } = useParams(); // grab listname from url
  const navigate = useNavigate();

  useEffect(() => {
    if (!listname) navigate("/home/activitysourcelist"); // load activitysourcelist if params not available
  }, []);

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
        {homepageActions &&
          homepageActions.map((action, index) => (
            <ActionCard
              key={index}
              title={action.title}
              href={`/home${action.href}`}
              active={listname === action.href.replace("/", "") && true}
            >
              <Text color="gray.600" fontSize="sm">
                {action.description}
              </Text>
            </ActionCard>
          ))}
      </HStack>
      {
        // conditional loading of the list based on listname
        listname === "activitysourcelist" ? (
          <ActivitySourceList /> // load Activity Source list if listname(params) is 'activitysourcelist'
        ) : (
          listname === "agentlist" && <AgentList /> // load Agent list if listname(params) is 'agentlist'
        )
      }
    </Stack>
  );
};
